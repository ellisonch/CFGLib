set -e

flex addition.lex
yacc -dtv addition.yacc
g++ -c lex.yy.c
g++ -c y.tab.c
g++ -o vars y.tab.o lex.yy.o